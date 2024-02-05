# Desafio BT – Inoa Sistemas
O desafio consiste em desenvolver um programa em C# que avisa, via e-mail, caso a cotação de um ativo da B3 caia mais do que certo nível, ou suba acima de outro.

## Premissas adotadas
  1.	Para consultar a cotação dos ativos, optei por utilizar a API da brapi. Entre as APIs gratuitas encontradas durante o desenvolvimento, a brapi destacou-se pela vantagem em termos de limite de requisições e frequência de atualização dos dados. Contudo, é importante notar que os dados possuem ao menos 30 minutos de atraso. Para iniciar a aplicação, o usuário deve gerar um token de acesso à API através do site: https://brapi.dev/.
  2.	É essencial que o usuário especifique um intervalo de tempo entre as requisições. Essa premissa foi adotada tendo em vista que o número de requisições permitidas é limitado. O plano gratuito da brapi oferece até 15.000 requisições por mês, permitindo ao usuário escolher um intervalo de tempo relativamente curto, se desejado. Além disso, caso seja necessário migrar para outra API no futuro, o intervalo de tempo pode ser facilmente ajustado para atender aos limites de requisições de outras APIs.
  3.	O código foi desenvolvido com o intuito de facilitar futuras modificações no sistema de requisições à API e notificações. Para fazer as requisições e notificações o programa depende de duas classes abstratas: uma para gerenciar requisições a APIs (APISystem) e outra para sistemas de notificações (NotificationSystem). Dessa forma, o programa pode ser facilmente adaptado para incluir outras bases de dados e diferentes tipos de notificações. Basta desenvolver essas alterações em classes derivadas de APISystem e NotificationSystem.
     
## Resumo do código
# O código foi separado em 4 classes:
    1.	**Program**: Responsável por preparar a aplicação para a execução, esta classe cria os objetos necessários, verifica os inputs do usuário e processa os parâmetros utilizados pelos objetos durante a execução. Após essas etapas, o método Alert de um objeto da classe StockQuoteAlert é chamado.
    2.	**StockQuoteAlert**: Encarregada de comparar a cotação do ativo com os preços de referência para compra e venda, esta classe toma decisões de notificação. Recebe como parâmetros dois objetos das classes APISystem e NotificationSystem, bem como o ativo que será monitorado, os valores de referência para compra e venda, e o tempo de intervalo entre as requisições. A classe cria as mensagens e, a partir do método Alert, entra em um loop para verificar a cotação do ativo, notificando quando necessário.
    3.	**APISystem**: Classe base criada para lidar com chamadas à APIs, contendo o método CheckPrice, responsável por retornar o valor da cotação do ativo. Durante o desenvolvimento do programa foi criada a classe derivada APIBrapi, especificamente para lidar com a API da brapi. Se ocorrer algum erro durante a requisição, o usuário será notificado e o programa encerrado.
    4.	**NotificationSystem**: Classe base criada para lidar com o envio de notificações, possuindo o método Notify para enviar algum tipo de notificação. Uma classe derivada, EmailNotification, foi criada utilizando como parâmetros as configurações de acesso ao servidor de SMTP e o e-mail de destino. 
    
## Como utilizar esta aplicação
  1.	Preencha o arquivo de configuração AppConfig.json com as configurações necessárias, incluindo as configurações de acesso ao servidor de SMTP, o e-mail de destino das notificações, seu token de acesso a API da brapi, e o intervalo de tempo entre as requisições (em segundos). 
  2.	Compile o código.
  3.	Execute o programa através da linha de comando, fornecendo os seguintes parâmetros: ativo a ser monitorado, preço de referência para venda e preço de referência para compra. 
  
  Exemplo:
  	desafioBT-INOA.exe PETR4 22.67 22.59

