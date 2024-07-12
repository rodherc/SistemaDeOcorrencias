-- TIPO DE OCORRENCIA
CREATE TABLE TIPO (
    ID BIGINT AUTO_INCREMENT PRIMARY KEY,
    CODIGO VARCHAR(3) NOT NULL,
    DESCRICAO VARCHAR(60) NOT NULL
);

INSERT INTO TIPO (CODIGO, DESCRICAO) VALUES
    ('03','RECUSA POR FALTA DE PEDIDO DE COMPRA'),
    ('04','RECUSA POR PEDIDO DE COMPRA CANCELADO'),
    ('500','INCIDENTE'),
    ('91','ENTREGA AGENDADA'),
    ('01','ENTREGA REALIZADA COM SUCESSO');

-- TRANSPORTADOR
CREATE TABLE TRANSPORTADOR (
    ID BIGINT AUTO_INCREMENT PRIMARY KEY,
    CNPJ VARCHAR(14) NOT NULL,
    DESCRICAO VARCHAR(60) NOT NULL
);

INSERT INTO TRANSPORTADOR (CNPJ, DESCRICAO) VALUES
    ('03279710000271','TOMASI'),
    ('05435749000185','ARGIUS'),
    ('11040609000291','HDLOG');

-- CADASTRO DA OCORRÊNCIA
CREATE TABLE OCORRENCIA (
    ID BIGINT AUTO_INCREMENT PRIMARY KEY,
    ID_TIPO BIGINT NOT NULL,
    ID_TRANSPORTADOR BIGINT NOT NULL,
    OCORREU_EM DATETIME NOT NULL,
    SOLUCAO_EM DATETIME NULL,
    FOREIGN KEY (ID_TIPO) REFERENCES TIPO(ID),
    FOREIGN KEY (ID_TRANSPORTADOR) REFERENCES TRANSPORTADOR(ID)
);

DELIMITER //

CREATE PROCEDURE popula_ocorrencia()
BEGIN
    DECLARE CONTADOR INT DEFAULT 1;
    WHILE CONTADOR < 1000 DO
        INSERT INTO OCORRENCIA (ID_TIPO, ID_TRANSPORTADOR, OCORREU_EM, SOLUCAO_EM)
        SELECT 
            A.ID,
            B.ID,
            DATE_SUB(NOW(), INTERVAL C.ALEATORIO SECOND),
            CASE WHEN CONTADOR % 2 = 0 THEN DATE_ADD(DATE_SUB(NOW(), INTERVAL C.ALEATORIO SECOND), INTERVAL 2 SECOND) ELSE NULL END
        FROM
            (SELECT ID FROM TIPO ORDER BY RAND() LIMIT 1) AS A
            CROSS JOIN (SELECT ID FROM TRANSPORTADOR ORDER BY RAND() LIMIT 1) AS B
            CROSS JOIN (SELECT FLOOR(RAND() * 14) AS ALEATORIO) AS C;
        
        SET CONTADOR = CONTADOR + 1;
    END WHILE;
END //

DELIMITER ;

CALL popula_ocorrencia();

CREATE VIEW view_ocorrencia_modal AS
SELECT 
    O.ID AS Numero,
    O.OCORREU_EM AS OcorreuEm,
    TIME(O.OCORREU_EM) AS Horario,
    O.SOLUCAO_EM AS Lancamento,
    T.CODIGO AS TipoCodigo,
    T.DESCRICAO AS TipoDescricao,
    TR.CNPJ AS TransportadoraCNPJ,
    TR.DESCRICAO AS TransportadoraDescricao,
    -- Campos adicionais que podem ser fixos ou calculados
    '296126-5' AS Documento, -- Exemplo de valor fixo
    '0000972686' AS Pedido, -- Exemplo de valor fixo
    'Sem infromação' AS Localizacao, -- Exemplo de valor fixo
    'Responsável pela Solução' AS ResponsavelSolucao, -- Exemplo de valor fixo
    CONCAT(TR.CNPJ, ' - CXS 101 - (CAXIAS DO SUL/RS)') AS Interessado, -- Exemplo de valor fixo
    'Responsável do Interessado' AS ResponsavelInteressado, -- Exemplo de valor fixo
    'SAC' AS OrigemInformacao, -- Exemplo de valor fixo
    'Contato' AS Contato, -- Exemplo de valor fixo
    '' AS RgCpf, -- Exemplo de valor fixo
    '' AS Responsavel, -- Exemplo de valor fixo
    'ADV - CPO' AS DepartamentoPadrao, -- Exemplo de valor fixo
    'Tomador de Serviço' AS UsuarioResponsavel, -- Exemplo de valor fixo
    'CAXIAS DO SUL' AS CidadeDestino, -- Exemplo de valor fixo
    'RIO GRANDE SO SUL' AS EstadoDestino, -- Exemplo de valor fixo
    'Observações' AS Observacoes -- Exemplo de valor fixo
FROM 
    OCORRENCIA O
JOIN 
    TIPO T ON O.ID_TIPO = T.ID
JOIN 
    TRANSPORTADOR TR ON O.ID_TRANSPORTADOR = TR.ID;
    