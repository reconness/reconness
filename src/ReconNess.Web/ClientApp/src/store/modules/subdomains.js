import api from '../../api'

const actions = {
    subdomain(context, { targetName, subdomain}) {
        return new Promise((resolve, reject) => {
            try {
                api.get('subdomains/' + targetName + '/' + subdomain)
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