import React, { Suspense, lazy, useContext } from 'react';
import { BrowserRouter, Route, Switch } from 'react-router-dom';

import GlobalSate from './components/context/GlobalState';
import ModuleContext from './components/context/ModuleContext';

import Layout from './components/Layout/Layout';
import Home from './components/pages/Home/Home';

const About = lazy(() => import('./components/pages/About/About'));
const Error404 = lazy(() => import('./components/pages/Error404/Error404'));

const App = () => {
    const context = useContext(ModuleContext);
    const { modules } = context;

    return (
        <GlobalSate>
            <BrowserRouter>
                <Layout>
                    <Suspense fallback={<div>Loading...</div>}>
                        <Switch>
                            <Route exact path='/' component={Home} />
                            <Route exact path='/about' component={About} />
                            {modules.map((module, index) => {

                                console.log(module);

                                let path = `./components/pages/products/${module.title}/${module.title}`;

                                console.log(path);

                                let moduleComponent = React.lazy(() =>
                                    import(`${path}`)
                                         .catch(() => ({ default: () => <div>Not found</div> }))
                                )

                                console.log(moduleComponent);

                                return (
                                    <Suspense key={index} fallback={<div>Loading...</div>}>
                                        <Route exact path={module.link} component={moduleComponent} />
                                    </Suspense>
                                );
                            })
                            }
                            <Route component={Error404} />
                        </Switch>
                    </Suspense>
                </Layout>
            </BrowserRouter>
        </GlobalSate>
    );
};

export default App;
