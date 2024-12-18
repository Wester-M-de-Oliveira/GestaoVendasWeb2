create table Estado (
    id int not null primary key auto_increment,
    nome varchar(200) not null,
    sigla varchar(2)
);

create table cidade (
    id int not null primary key auto_increment,
    nome varchar(200) not null,
    estado_id int,
    foreign key (estado_id) references Estado (id)
);

create table Endereco (
    id integer not null primary key auto_increment,
    rua varchar(300),
    numero integer,
    bairro varchar(100),
    cidade_id int not null,
    foreign key (cidade_id) references Cidade (id)
);

create table Cliente (
    id integer not null primary key auto_increment,
    nome varchar(200) not null,
    estado_civil varchar(50),
    cpf varchar(20) not null,
    rg varchar(30),
    data_nasc date,
    renda_familiar double,
    telefone varchar(50),
    sexo varchar(20) not null,
    celular varchar(50) not null,
    endereco_id integer not null,
    foreign key (endereco_id) references Endereco (id)
);

create table Funcionario (
    id integer not null primary key auto_increment,
    nome varchar(200) not null,
    cpf varchar(20) not null,
    rg varchar(20),
    data_nasc date,
    salario double not null,
    telefone varchar(50),
    funcao varchar(50) not null,
    sexo varchar(20) not null,
    endereco_id integer not null,
    foreign key (endereco_id) references Endereco (id)
);

create table Fornecedor (
    id integer not null primary key auto_increment,
    razao_social varchar(200),
    nome_fantasia varchar(100),
    endereco_id integer not null,
    foreign key (endereco_id) references Endereco (id)
);

create table Produto (
    id int primary key not null auto_increment,
    nome varchar(50) not null,
    descricao varchar(50) not null,
    preco_compra double,
    valor_prod double,
    quantidade_estoque int,
    data_validade date
);

create table Compra (
    id int not null primary key auto_increment,
    data date,
    valor double,
    forma_pag varchar(100),
    funcionario_id int not null,
    fornecedor_id int not null,
    foreign key (funcionario_id) references Funcionario (id),
    foreign key (fornecedor_id) references Fornecedor (id)
);

create table Itens_Compra (
    id int not null primary key auto_increment,
    quantidade int not null,
    valor double not null,
    produto_id int not null,
    compra_id int not null,
    foreign key (produto_id) references Produto (id),
    foreign key (compra_id) references Compra (id)
);

create table Caixa (
    id int not null primary key auto_increment,
    data_abertura date not null,
    data_fechamento date,
    saldo_inicial double not null,
    troco double,
    valor_creditos double,
    valor_debitos double,
    saldo_final double,
    status varchar(100) not null
);

create table Venda (
    id int not null primary key auto_increment,
    data_venda date,
    valor double not null,
    desconto double,
    forma_pag varchar(50),
    quant_parcelas int,
    funcionario_id int not null,
    cliente_id int not null,
    foreign key (funcionario_id) references Funcionario (id),
    foreign key (cliente_id) references Cliente (id)
);

create table Itens_Venda (
    id int not null primary key auto_increment,
    quantidade int not null,
    valor double,
    produto_id int not null,
    venda_id int not null,
    foreign key (produto_id) references Produto (id),
    foreign key (venda_id) references Venda (id)
);

create table Recebimentos (
    id int not null primary key auto_increment,
    data date,
    valor double,
    caixa_id int,
    venda_id int,
    funcionario_id int not null,
    foreign key (caixa_id) references Caixa (id),
    foreign key (venda_id) references Venda (id),
    foreign key (funcionario_id) references Funcionario (id)
);

create table Despesas (
    id int not null primary key auto_increment,
    descricao varchar(200),
    valor double,
    data_vencimento date,
    numero_doc int,
    fornecedor_id int,
    foreign key (fornecedor_id) references Fornecedor (id)
);

create table Pagamentos (
    id int not null primary key auto_increment,
    data date,
    valor double,
    forma_pag varchar(100),
    caixa_id int,
    compra_id int,
    despesa_id int,
    funcionario_id int,
    foreign key (caixa_id) references Caixa (id),
    foreign key (despesa_id) references Despesas (id),
    foreign key (compra_id) references Compra (id),
    foreign key (funcionario_id) references Funcionario (id)
);