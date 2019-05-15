import React, { useState, useEffect } from 'react';
import ModuleContext from './ModuleContext';

const GlobalState = props => {
    const [modules, setModules] = useState([]);

    const [status, setStatus] = useState({
        error: "",
        isFetching: true
    });

    // componentDidMount hook
    useEffect(() => {
        fetch('http://localhost:8080/api/modules')
            .then(response => response.json())
            .then(result => { 
                setModules(result);
                setStatus({
                    error: "",
                    isFetching: false
                });
            })
            .catch (e => 
                {
                console.log(e);
                setModules([]);
                setStatus({
                    error: e,
                    isFetching: false
                });  
            });
    }, []);

    return (
        <ModuleContext.Provider value={{modules: modules, status: status}}>
            {props.children}
        </ModuleContext.Provider>
    );
}

export default GlobalState;