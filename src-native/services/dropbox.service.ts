import { Constants, MessageModel, MessageType, ProviderModel, Helpers } from '../../src-common';
import { IpcService } from './ipc.service';
import { SuperService } from '../super.service';

export class DropboxService extends SuperService {
    // private readonly ACCESS_TOKEN = 'JYfV_JpVuKMAAAAAAAADcLpyn2bzlTGiWOhMF7zIRmvoTq1zKRvSZQOLI5oWE_7E';
    
    constructor(private readonly _ipc: IpcService) {
        super();
        this._ipc.Receive.on(Constants.IPC_CHANNEL, (message: MessageModel) => this.OnMessage(message));
    }

    protected async Dispose(): Promise<void> {
        this._ipc.Receive.removeAllListeners(Constants.IPC_CHANNEL);
    }

    private async GetProvider(): Promise<ProviderModel> {
        let provider = ProviderModel.New(`Dropbox`, 'fab fa-dropbox');
        return new Promise<ProviderModel>((resolve, reject) => {
            if (provider)
                resolve(provider);
            else
                reject('Failed to get Dropbox provider.');
        });
    }

    private OnMessage(message: MessageModel): void {
        switch (message.Type) {
            case MessageType.DropboxProvider:
                this.GetProvider().then(provider => {
                    message.DataJson = Helpers.Serialize<ProviderModel>(provider);
                    this._ipc.Send(message);
                }).catch(ex =>
                    console.log(ex)
                );
                break;
        }
    }
}