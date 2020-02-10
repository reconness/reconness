import api from '../../api'

const actions = {
    login(context, { username, password}) {
        return new Promise((resolve, reject) => {
            try {
                api.login(username, password)
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    }
}


export default {
    namespaced: true,
    actions
}