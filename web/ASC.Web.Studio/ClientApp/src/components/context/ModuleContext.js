import React from 'react';

const ModuleContext = React.createContext({
    modules: [
        { title: "People", link: "/products/people/", image: "people_logolarge.png", description: "Manage portal employees.", isPrimary: true },
    ],
    isFetching: true
 });

export default ModuleContext;