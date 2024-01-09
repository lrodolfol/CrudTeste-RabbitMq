namespace CreateUser.API.Model.Response;

public class ViewResponse
{
    public ViewResponse(){}
    public ViewResponse(List<string> erros, short code = 404) =>
        (Erros, Code) = (erros, code);
    public ViewResponse(object data, short code = 201) =>
        (Data, Code) = (data, code);
    
    public short Code { get; set; } = 202;
    public List<string> Erros { get; set; } = new ();
    public object Data { get; set; } = null!;    

    public void AddData(object data, short code = 202) => (Data, Code) = (data, code);
    public void AddErros(List<string> erros, short code = 404) => (Erros, Code) = (erros, code);
    public void AddErros(string erro, short code = 404)
    {
        Erros.Add(erro);
        Code = code;
    }
}
