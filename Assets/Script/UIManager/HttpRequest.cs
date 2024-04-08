using System.Collections;

public interface HttpRequest
{
    public IEnumerator PostReq(string url, string data);
}
