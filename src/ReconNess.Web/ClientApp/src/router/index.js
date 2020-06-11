import Vue from 'vue';
import Router from 'vue-router';

import HomePage from '@/views/home/HomePage';
import SettingPage from '@/views/account/SettingPage';
import LoginPage from '@/views/login/LoginPage'

import TargetPage from '@/views/target/TargetPage';
import RootDomainPage from '@/views/target/RootDomainPage';
import TargetCreatePage from '@/views/target/TargetCreatePage';

import AgentCreatePage from '@/views/agent/AgentCreatePage';
import AgentEditPage from '@/views/agent/AgentEditPage';
import AgentDebugPage from '@/views/agent/AgentDebugPage';
import AgentInstallPage from '@/views/agent/AgentInstallPage';

import SubdomainPage from '@/views/subdomain/SubdomainPage';

Vue.use(Router);

const router = new Router({
  routes: [
        { path: '/', name: 'home', component: HomePage },
        { path: '/settings', name: 'setting', component: SettingPage },

        { path: '/login', name: 'login', component: LoginPage },

        { path: '/targets/create', name: 'targetCreate', component: TargetCreatePage },
        { path: '/targets/:targetName/:rootDomain', name: 'targetRootDomain', component: RootDomainPage },
        { path: '/targets/:targetName', name: 'target', component: TargetPage },        

        { path: '/agents/create', name: 'agentCreate', component: AgentCreatePage },
        { path: '/agents/debug', name: 'agentDebug', component: AgentDebugPage },  
        { path: '/agents/install', name: 'installDebug', component: AgentInstallPage }, 
        { path: '/agents/:agentName', name: 'agentEdit', component: AgentEditPage },                

        { path: '/subdomains/:targetName/:rootDomain/:subdomain', name: 'subdomain', component: SubdomainPage },

        { path: '*', redirect: { name: 'home' } },
  ],
});

router.beforeEach((to, from, next) => {
    // redirect to login page if not logged in and trying to access a restricted page
    const publicPages = ['/login'];
    const authRequired = !publicPages.includes(to.path);
    const loggedIn = localStorage.getItem('user'); // TODO: I need to verify if the token is valid too

    if (authRequired && !loggedIn) {
        return next('/login');
    }

    next();
})

export default router