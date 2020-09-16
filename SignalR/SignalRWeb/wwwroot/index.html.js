//WebSocket = undefined;
//EventSource = undefined;
//, signalR.HttpTransportType.LongPolling
const LOCAL_BASE_URL = 'https://localhost:44361';
const REMOTE_BASE_URL = '<WEB_APP_ENDPOINT>';

const getAPIBaseUrl = () => {
    const isLocal = /localhost/.test(window.location.href);
    return isLocal ? LOCAL_BASE_URL : REMOTE_BASE_URL;
}
const app = new Vue({
    el: '#app',
    data() {
        return {
            items: []
        }
    },
    methods: {
        async getItems() {
            try {
                const apiUrl = `${getAPIBaseUrl()}/api/content`;
                const response = await axios.get(apiUrl);
                app.items = response.data;
            } catch (ex) {
                console.error(ex);
            }
        }
    },
    created() {
        //this.getItems();
    }
});
let connection = null;

setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/contenthub")
        .build();

    connection.on("Items", (data) => {
            app.items = data;
        }
    );
    connection.start().catch(function (e) {
    });
};

setupConnection();
