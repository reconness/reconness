import Vue from 'vue';
import Router from 'vue-router';

import HomePage from '@/views/home/HomePage';
import NotificationPage from '@/views/account/NotificationPage';
import LogsPage from '@/views/account/LogsPage';

import UserListPage from '@/views/account/UserListPage';
import UserCreatePage from '@/views/account/UserCreatePage';
import UserDetailPage from '@/views/account/UserDetailPage';


import LoginPage from '@/views/login/LoginPage'

import TargetPage from '@/views/target/TargetPage';
import RootDomainPage from '@/views/rootdomain/RootDomainPage';
import TargetCreatePage from '@/views/target/TargetCreatePage';

import AgentCreatePage from '@/views/agent/AgentCreatePage';
import AgentEditPage from '@/views/agent/AgentEditPage';
import AgentDebugPage from '@/views/agent/AgentDebugPage';
import WordlistPage from '@/views/agent/WordlistPage';
import AgentInstallPage from '@/views/agent/AgentInstallPage';

import SubdomainPage from '@/views/subdomain/SubdomainPage';

Vue.use(Router);

const router = new Router({
    routes: [
        { path: '/', name: 'home', component: HomePage },
        { path: '/notifications', name: 'notifications', component: NotificationPage },
        { path: '/logs', name: 'logs', component: LogsPage },

        { path: '/users', name: 'user', component: UserListPage },
        { path: '/users/:id', name: 'userDetails', component: UserDetailPage },
        { path: '/users/create', name: 'userCreate', component: UserCreatePage },       

        { path: '/login', name: 'login', component: LoginPage },

        { path: '/targets/create', name: 'targetCreate', component: TargetCreatePage },
        { path: '/targets/:targetName/:rootDomain', name: 'targetRootDomain', component: RootDomainPage },
        { path: '/targets/:targetName', name: 'target', component: TargetPage },

        { path: '/agents/create', name: 'agentCreate', component: AgentCreatePage },
        { path: '/agents/debug', name: 'agentDebug', component: AgentDebugPage },
        { path: '/agents/wordlists', name: 'wordlist', component: WordlistPage },
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