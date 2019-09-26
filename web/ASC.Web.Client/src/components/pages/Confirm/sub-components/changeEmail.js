import React from 'react';
import { withRouter } from "react-router";
import { withTranslation } from 'react-i18next';
import { PageLayout, Loader } from 'asc-web-components';
import { connect } from 'react-redux';
import { logout, changeEmail } from '../../../../store/auth/actions';
import PropTypes from 'prop-types';


class ChangeEmail extends React.PureComponent {

    constructor(props) {
        super(props);
        this.state = {
            queryString: props.location.search.slice(1)
        };
    }

    componentDidUpdate(){
        const { logout, changeEmail, userId, isLoaded } = this.props;
        if (isLoaded){
        const queryParams = this.state.queryString.split('&');
        const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
        const linkParams = Object.fromEntries(arrayOfQueryParams);
        // logout();
        const email = decodeURIComponent(linkParams.email);
        changeEmail(userId, {email}, this.state.queryString)
            .then((res) => {
                console.log('change client email success', res)
                window.location.href = `${window.location.origin}/products/people/view/@self?email_change=success`;
            })
            .catch((e) => {
                console.log('change client email error', e)
            });
        }
    }

    render() {
        console.log('Change email render');
        return (
            <Loader className="pageLoader" type="rombs" size={40} />
        );
    }
}



ChangeEmail.propTypes = {
    location: PropTypes.object.isRequired,
    history: PropTypes.object.isRequired
};
const ChangeEmailForm = (props) => (<PageLayout sectionBodyContent={<ChangeEmail {...props} />} />);

function mapStateToProps(state) {
    return {
        isLoaded: state.auth.isLoaded,
        userId: state.auth.user.id
    };
}

export default connect(mapStateToProps, { logout, changeEmail })(withRouter(withTranslation()(ChangeEmailForm)));