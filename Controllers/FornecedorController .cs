
    [Route("fornecedores")]
    [ApiController]
    public class FornecedorController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Fornecedor> fornecedores = new FornecedorDAO().List();
                return Ok(fornecedores);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var fornecedor = new FornecedorDAO().GetById(id);

                if (fornecedor == null) return NotFound();

                return Ok(fornecedor);
            }
            catch (Exception)
            {
                return Problem("Erro ao processar a solicitação.");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] FornecedorDTO item)
        {
            var fornecedor = new Fornecedor
            {
                Nome = item.Nome,
                Telefone = item.Telefone,
                Cidade = item.Cidade,
                Endereco = item.Endereco
            };

            try
            {
                var dao = new FornecedorDAO();
                fornecedor.IdFornecedor = dao.Insert(fornecedor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created("", fornecedor);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] FornecedorDTO item)
        {
            try
            {
                var fornecedor = new FornecedorDAO().GetById(id);
                if (fornecedor == null) return NotFound();

                fornecedor.Nome = item.Nome;
                fornecedor.Telefone = item.Telefone;
                fornecedor.Cidade = item.Cidade;
                fornecedor.Endereco = item.Endereco;

                new FornecedorDAO().Update(fornecedor);

                return Ok(fornecedor);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var fornecedor = new FornecedorDAO().GetById(id);
                if (fornecedor == null) return NotFound();

                new FornecedorDAO().Delete(fornecedor.IdFornecedor);

                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
