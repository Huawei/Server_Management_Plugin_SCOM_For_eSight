DIM oAPI, oBag, status
Set oAPI = CreateObject("MOM.ScriptAPI")
Set oBag = oAPI.CreatePropertyBag()
status = WScript.Arguments(0)
Call oBag.AddValue("healthStatus", status)
Call oAPI.Return(oBag)
