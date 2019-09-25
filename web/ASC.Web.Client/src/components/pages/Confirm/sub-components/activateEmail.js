import React from 'react';
import { withRouter } from "react-router";
import { withTranslation } from 'react-i18next';
import { PageLayout, Loader } from 'asc-web-components';
import { connect } from 'react-redux';
import { logout, validateActivatingEmail } from '../../../../store/auth/actions';
import PropTypes from 'prop-types';


class ActivateEmail extends React.PureComponent {

    constructor(props) {
        super(props);
        this.state = {
            queryString: `type=EmailActivation&${props.location.search.slice(1)}`
        };
    }

    componentDidMount() {
        const { history, logout, validateActivatingEmail } = this.props;
        const queryParams = this.state.queryString.split('&');
        const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
        const linkParams = Object.fromEntries(arrayOfQueryParams);
        logout();
        validateActivatingEmail(linkParams)
            .then((res) => {
                const email = decodeURIComponent(res.data.response.email);
                history.push(`/login/confirmed-email=${email}`);
                
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


export default connect(null, { logout, validateActivatingEmail })(withRouter(withTranslation()(ActivateEmailForm)));