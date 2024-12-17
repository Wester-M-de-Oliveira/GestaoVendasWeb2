create table Produto (
    id int primary key not null auto_increment,
    nome varchar(50) not null,
    descricao varchar(50) not null,
    preco float,
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
    valor float not null,
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
    valor float,
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