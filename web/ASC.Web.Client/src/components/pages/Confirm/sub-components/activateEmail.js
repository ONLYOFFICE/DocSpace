import React from 'react';
import { withRouter } from "react-router";
import { withTranslation } from 'react-i18next';
import { PageLayout, Loader } from 'asc-web-components';
import { connect } from 'react-redux';
import { logout, changeEmail } from '../../../../store/auth/actions';
import PropTypes from 'prop-types';


class ActivateEmail extends React.PureComponent {

    constructor(props) {
        super(props);
        this.state = {
            queryString: `type=EmailActivation&${props.location.search.slice(1)}`
        };
    }

    componentDidMount() {
        const { history, logout, changeEmail } = this.props;
        const queryParams = this.state.queryString.split('&');
        const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
        const linkParams = Object.fromEntries(arrayOfQueryParams);
        const email = decodeURIComponent(linkParams.email);
        const uid = linkParams.uid;
        // logout();
        changeEmail(uid, { email }, this.state.queryString)
            .then((res) => {
                history.push(`/login/confirmed-email=${email}`);
            })
            .catch((e) => {
                console.log('activate email error', e);
                history.push(`/login/error=${e}`);
            });
    }

    render() {
        console.log('Activate email render');
        return (
            <Loader className="pageLoader" type="rombs" size={40} />
        );
    }
}



ActivateEmail.propTypes = {
    location: PropTypes.object.isRequired,
    history: PropTypes.object.isRequired
};
const ActivateEmailForm = (props) => (<PageLayout sectionBodyContent={<ActivateEmail {...props} />} />);


export default connect(null, { logout, changeEmail })(withRouter(withTranslation()(ActivateEmailForm)));