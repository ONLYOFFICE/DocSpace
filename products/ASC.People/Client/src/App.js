import React, { Suspense, lazy } from 'react';
import { BrowserRouter, Switch } from 'react-router-dom';
import { Loader, ErrorContainer } from 'asc-web-components';
import PeopleLayout from './components/Layout';
import Home from './components/pages/Home';
import { PrivateRoute } from './helpers/privateRoute';
var config = require('../package.json');

const Profile = lazy(() => import('./components/pages/Profile'));
const ProfileAction = lazy(() => import('./components/pages/ProfileAction'));

const App = () => {
    return (
        <BrowserRouter>
            <PeopleLayout>
                <Suspense fallback={<Loader className="pageLoader" type="rombs" size={40} />}>
                    <Switch>
                        <PrivateRoute exact path={config.homepage} component={Home} />
                        <PrivateRoute exact path={`${config.homepage}/profile`} component={Profile} />
                        <PrivateRoute exact path={`${config.homepage}/profileaction`} component={ProfileAction} />
                        <PrivateRoute component={() => (
                            <ErrorContainer>
                                Sorry, the resource
                                cannot be found.
                            </ErrorContainer>
                        )} />
                    </Switch>
                </Suspense>
            </PeopleLayout>
        </BrowserRouter>
    );
};

export default App;
