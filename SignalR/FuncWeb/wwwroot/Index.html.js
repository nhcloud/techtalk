//WebSocket = undefined;
const LOCAL_BASE_URL = 'http://localhost:7071';
const REMOTE_BASE_URL = '<FUNCTION_APP_ENDPOINT>';

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
                const apiUrl = `${getAPIBaseUrl()}/api/data`;
                const response = await axios.get(apiUrl);
                app.items = response.data;
            } catch (ex) {
                console.error(ex);
            }
        }
    },
    created() {
        this.getItems();
    }
});

const connect = () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${getAPIBaseUrl()}/api`)
        .build();

    connection.onclose(() => {
        console.log('SignalR connection disconnected');
        setTimeout(() => connect(), 2000);
    });

    connection.on('updated', updatedItem => {
        const index = app.items.findIndex(s => s.id === updatedItem[0].id);
        app.items.splice(index, 1, updatedItem[0]);
    });

    connection.start().then(() => {
        console.log("SignalR connection established");
    });
};

connect();