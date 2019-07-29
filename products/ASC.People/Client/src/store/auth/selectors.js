export function getAvailableModules(modules) {
    const separator = { seporator: true, id: 'nav-seporator-1' };
    const products = modules.map(product => {
        return {
            id: product.id,
            title: product.title,
            iconName: 'PeopleIcon',
            notifications: 0,
            url: product.link,
            onClick: () => window.open(product.link, '_self'),
            onBadgeClick: e => console.log('PeopleIconBadge Clicked', e)
        };
    }) || [];

    return products.length ? [separator, ...products] : products;
};

export function isAdmin(auth) {
    return auth.user.isAdmin || auth.user.isOwner;
};

export function isMe(auth, userId) {
    return userId === "@self" || userId === auth.user.userName;
};