import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import { AUTH_KEY } from './constants';
import Cookies from 'universal-cookie';
import { connect } from 'react-redux';
import { checkConfirmLink } from './../store/auth/actions';
import { ValidationResult } from './../helpers/constants';
import decomposeConfirmLink from './../helpers/decomposeConfirmLink';
import { PageLayout, Loader } from "asc-web-components";

class ConfirmRoute extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            type: '',
            key: '',
            p: '',
            emplType: '',
            email: '',
            uid: '',
            firstname: '',
            lastname: '',
            isReady: false,
            componentProps: {}
        }
    }

    componentDidMount() {
        const { pathname, search } = this.props.location;
        const { checkConfirmLink } = this.props;
        const decomposedLink = decomposeConfirmLink(pathname, search);
        let validationResult;
        checkConfirmLink(decomposedLink)
            .then((res) => {
                validationResult = res.data.response;
                switch (validationResult) {
                    case ValidationResult.Ok:
                        const confirmHeader = `type=${decomposedLink.type}&${search.slice(1)}`;
                        const componentProps = Object.assign({}, decomposedLink, { confirmHeader });
                        this.setState({
                            isReady: true,
                            componentProps
                        });
                        break;
                    case ValidationResult.Invalid:
                        window.location.href = '/login/error=Invalid link'
                        break;
                    case ValidationResult.Expired:
                        window.location.href = '/login/error=Expired link'
                        break;
                    default:
                        window.location.href = '/login/error=Unknown error'
                        break;
                }
            })
            .catch((e) => window.location.href = '/');
    }

    render() {
        const { component: Component, location, path, computedMatch, ...rest } = this.props;
        const newProps = Object.assign({}, { location, path, computedMatch }, { linkData: this.state.componentProps });
        return (
            <Route
                {...rest}
                render={props =>
                    !this.state.isReady ? (
                        <PageLayout
                            sectionBodyContent={
                                <Loader className="pageLoader" type="rombs" size={40} />
                            }
                        />
                    ) : (
                                <Component {...newProps} />
                            )
                }
            />
        )
    }
};

export default connect(null, { checkConfirmLink })(ConfirmRoute);
