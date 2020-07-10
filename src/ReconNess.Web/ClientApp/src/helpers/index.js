
export default {
    errorHandle(error) {
        if (error) {
            alert(JSON.stringify(error))
        }
        else {
            alert("We had some issues. Try again, please!")
        }
    }
}