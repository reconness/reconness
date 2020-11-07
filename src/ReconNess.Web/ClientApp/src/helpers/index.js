
export default {
    errorHandle(alert, error) {
        if (error) {
            alert(JSON.stringify(error), "Error", 'error')
        }
        else {
            alert("We had some issues. Try again, please!", "Error",'error')
        }
    }
}