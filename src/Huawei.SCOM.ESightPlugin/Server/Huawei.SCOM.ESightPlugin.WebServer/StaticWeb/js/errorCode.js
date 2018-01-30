var errorCode_en = {
    //-------------------自定义---------------------------------------------
    "0": "Successful",
    "1": "The username of eSight does not match the password or the account does not exist.",
    "1204": "eSight connection failed. please try again.",
    "-331": "eSight connection failed. please try again.",
    "201": "query or delete failed, the object to be operated does not exist",
    "1001": "eSight license has expired!",
    "1002": "eSight license does not have the function to call openapi",
    "1024": "eSight authentication failed",

    "149201": "License number is insufficient.",
    "-99999": "OperationFailed: unknownerror",
    "-90001": "System internal error, please initialize",
    "-90002": "Please configure eSight first",
    "-90003": "Login status effectiveness, Please login vCenter again",
    "-90004": "Now there are no failed tasks that can be cleared",
    "-90005": "The user name of eSight does not match the password or the account does not exist.",
    "-90006": "The esight has already exist.",
    "-80009": "Failed to connect current eSight:A error occurred when connecting to the eSight server.",
    "-80010": "Failed to connect current eSight:An error occurred when connecting to the eSight server. No connection could be made because the target machine actively refused.",
    "-80011": "Sytem can not find this eSight",
    "-81111": "Update Error:Can not find this eSight",
    "-81001": "Failed to connect current eSight:An error occurred when connecting to the eSight server. Network connection error.",
    "-70001": "No data found in the database.",
    //http
    "-50400": "Failed to connect current eSight:BadRequest indicates that the request could not be understood by the server. BadRequest is sent when no other error is applicable, or if the exact error is unknown or does not have its own error code.",
    "-50401": "Failed to connect current eSight:Unauthorized indicates that the requested resource requires authentication. The WWW-Authenticate header contains the details of how to perform the authentication.",
    "-50402": "Failed to connect current eSight:PaymentRequired is reserved for future use.",
    "-50403": "Failed to connect current eSight:Forbidden indicates that the server refuses to fulfill the request.",
    "-50404": "Failed to connect current eSight:NotFound indicates that the requested resource does not exist on the server.",
    "-50405": "Failed to connect current eSight:MethodNotAllowed indicates that the request method (POST or GET) is not allowed on the requested resource.",
    "-50406": "Failed to connect current eSight:NotAcceptable indicates that the client has indicated with Accept headers that it will not accept any of the available representations of the resource.",
    "-50407": "Failed to connect current eSight:ProxyAuthenticationRequired indicates that the requested proxy requires authentication. The Proxy-authenticate header contains the details of how to perform the authentication.",
    "-50408": "Failed to connect current eSight:RequestTimeout indicates that the client did not send a request within the time the server was expecting the request.",
    "-50409": "Failed to connect current eSight:Conflict indicates that the request could not be carried out because of a conflict on the server.",
    "-50410": "Failed to connect current eSight:Gone indicates that the requested resource is no longer available.",
    "-50411": "Failed to connect current eSight:LengthRequired indicates that the required Content-length header is missing.",
    "-50412": "Failed to connect current eSight:PreconditionFailed indicates that a condition set for this request failed, and the request cannot be carried out. Conditions are set with conditional request headers like If-Match, If-None-Match, or If-Unmodified-Since.",
    "-50413": "Failed to connect current eSight:RequestEntityTooLarge indicates that the request is too large for the server to process.",
    "-50414": "Failed to connect current eSight:RequestUriTooLong indicates that the URI is too long.",
    "-50415": "Failed to connect current eSight:UnsupportedMediaType indicates that the request is an unsupported type.",
    "-50416": "Failed to connect current eSight:RequestedRangeNotSatisfiable indicates that the range of data requested from the resource cannot be returned, either because the beginning of the range is before the beginning of the resource, or the end of the range is after the end of the resource.",
    "-50417": "Failed to connect current eSight:ExpectationFailed indicates that an expectation given in an Expect header could not be met by the server.",
    "-50500": "Failed to connect current eSight:InternalServerError indicates that a generic error has occurred on the server.",
    "-50501": "Failed to connect current eSight:NotImplemented indicates that the server does not support the requested function.",
    "-50502": "Failed to connect current eSight:BadGateway indicates that an intermediate proxy server received a bad response from another proxy or the origin server.",
    "-50503": "Failed to connect current eSight:ServiceUnavailable indicates that the server is temporarily unavailable, usually due to high load or maintenance.",
    "-50504": "Failed to connect current eSight:GatewayTimeout indicates that an intermediate proxy server timed out while waiting for a response from another proxy or the origin server.",
    "-50505": "Failed to connect current eSight:HttpVersionNotSupported indicates that the requested HTTP version is not supported by the server."
}
/**
 * 获取错误代码
 * @param  errorCode 
 * @returns  errorMsg
 */
function getErrorMsg(errorCode) {
    var lang = localStorage.getItem('lang');
    if (errorCode_en[errorCode]) {
        return errorCode_en[errorCode];
    }
    return errorCode;
};