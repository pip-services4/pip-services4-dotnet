.PHONY: all build clean install uninstall fmt simplify check run test protogen docgen

env:
	
test: env
	dotnet test test/test.csproj

protogen: env
	protoc --proto_path=src/Protos --csharp_out=src/Protos --csharp_opt=file_extension=.cs,base_namespace=PipServices4.Grpc.Services src/Protos/commandable.proto
	protoc --proto_path=example/Protos --csharp_out=example/Protos --csharp_opt=file_extension=.cs,base_namespace=PipServices4.Grpc.Protos example/Protos/dummies.proto