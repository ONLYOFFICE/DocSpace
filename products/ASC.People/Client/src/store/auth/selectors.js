export function getAvailableModules(modules) {
    const separator = { seporator: true, id: 'nav-seporator-1' };
    const chat = {
        id: '22222222-2222-2222-2222-222222222222',
        title: 'Chat',
        iconName: 'ChatIcon',
        notifications: 3,
        url: '/products/chat/',
        onClick: () => window.open('/products/chat/', '_self'),
        onBadgeClick: e => console.log('ChatIconBadge Clicked', e),
        isolateMode: true
    };

    const items = modules.map(item => {
        return {
            id: '11111111-1111-1111-1111-111111111111',
            title: item.title,
            iconName: 'PeopleIcon',
            notifications: 0,
            url: item.link,
            onClick: () => window.open(item.link, '_self'),
            onBadgeClick: e => console.log('PeopleIconBadge Clicked', e)
        };
    }) || [];

    return items.length ? [separator, ...items, chat] : items;
};

export function isAdmin(auth) {
    return auth.user.isAdmin || auth.user.isOwner;
};

export function isMe(auth, userId) {
    return userId === "@self" || userId === auth.user.userName;
};