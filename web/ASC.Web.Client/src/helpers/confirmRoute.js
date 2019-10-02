import React from 'react';
import { Route } from 'react-router-dom';
import { ValidationResult } from './../helpers/constants';
import { decomposeConfirmLink } from './../helpers/converters';
import { PageLayout, Loader } from "asc-web-components";
import { connect } from 'react-redux';
import { checkConfirmLink } from './../store/auth/actions';
import { withRouter } from "react-router";

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
            isLoaded: false,
            componentProps: {}
        }
    }

    componentDidMount() {
        const { location, checkConfirmLink, isAuthenticated, history } = this.props;
        const { search } = location;
        const decomposedLink = decomposeConfirmLink(location);
        let validationResult;
        let path = '';
        if (!isAuthenticated) {
            path = '/login';
        }
        checkConfirmLink(decomposedLink)
            .then((res) => {
                validationResult = res.data.response;
                switch (validationResult) {
                    case ValidationResult.Ok:
                        const confirmHeader = `type=${decomposedLink.type}&${search.slice(1)}`;
                        const componentProps = Object.assign({}, decomposedLink, { confirmHeader });
                        this.setState({
                            isLoaded: true,
                            componentProps
                        });
                        break;
                    case ValidationResult.Invalid:
                        history.push(`${path}/error=Invalid link`);
                        break;
                    case ValidationResult.Expired:
                        history.push(`${path}/error=Expired link`);
                        break;
                    default:
                        history.push(`${path}/error=Unknown error`);
                        break;
                }
            })
            .catch((e) => history.push(`${path}/error=${e}`));
    }

    render() {
        const { component: Component, ...rest } = this.props;
        return (
            <Route
                {...rest}
                render={props =>
                    !this.state.isLoaded ? (
                        <PageLayout
                            sectionBodyContent={
                                <Loader className="pageLoader" type="rombs" size={40} />
                            }
                        />
                    ) : (
                            <Component {...props = { ...props, linkData: this.state.componentProps }} />
                        )
                }
            />
        )
    }
};

function mapStateToProps(state) {
    return {
        isAuthenticated: state.auth.isAuthenticated
    };
}

export default connect(mapStateToProps, { checkConfirmLink })(withRouter(ConfirmRoute));
