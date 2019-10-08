import React from 'react';
import { withRouter } from "react-router";
import { withTranslation } from 'react-i18next';
import { PageLayout, Loader } from 'asc-web-components';
import { connect } from 'react-redux';
import { changeEmail } from '../../../../store/auth/actions';
import PropTypes from 'prop-types';


class ChangeEmail extends React.PureComponent {
    componentDidMount() {
        const { changeEmail, userId, isLoaded, linkData } = this.props;
        if (isLoaded) {
            const [email, key] = [linkData.email, linkData.confirmHeader];
            changeEmail(userId, email , key)
                .then((res) => {
                    console.log('change client email success', res)
                    window.location.href = `${window.location.origin}/products/people/view/@self?email_change=success`;
                })
                .catch((e) => {
                    console.log('change client email error', e)
                    window.location.href = `${window.location.origin}/error=${e.message}`;
                });
        }
    }

    componentDidUpdate() {
        const { changeEmail, userId, isLoaded, linkData } = this.props;
        if (isLoaded) {
            const [email, key] = [linkData.email, linkData.confirmHeader];
            changeEmail(userId, email, key)
                .then((res) => {
                    console.log('change client email success', res)
                    window.location.href = `${window.location.origin}/products/people/view/@self?email_change=success`;
                })
                .catch((e) => {
                    console.log('change client email error', e)
                });
        } else {
            window.location.href = '/';
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
    changeEmail: PropTypes.func.isRequired
};
const ChangeEmailForm = (props) => (<PageLayout sectionBodyContent={<ChangeEmail {...props} />} />);

function mapStateToProps(state) {
    return {
        isLoaded: state.auth.isLoaded,
        userId: state.auth.user.id
    };
}

export default connect(mapStateToProps, { changeEmail })(withRouter(withTranslation()(ChangeEmailForm)));