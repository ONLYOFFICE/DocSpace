import React from "react";
import { withRouter } from "react-router";
import { Loader } from "asc-web-components";
import { PageLayout, utils as commonUtils } from "asc-web-common";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
const { tryRedirectTo } = commonUtils;

class ChangeEmail extends React.PureComponent {
  componentDidMount() {
    const { changeEmail, userId, isLoaded, linkData } = this.props;
    if (isLoaded) {
      const [email, key] = [linkData.email, linkData.confirmHeader];
      changeEmail(userId, email, key)
        .then((res) => {
          console.log("change client email success", res);
          tryRedirectTo(
            `${window.location.origin}/products/people/view/@self?email_change=success`
          );
        })
        .catch((e) => {
          console.log("change client email error", e);
          tryRedirectTo(`${window.location.origin}/error=${e}`);
        });
    }
  }

  componentDidUpdate() {
    const { changeEmail, userId, isLoaded, linkData, defaultPage } = this.props;
    if (isLoaded) {
      const [email, key] = [linkData.email, linkData.confirmHeader];
      changeEmail(userId, email, key)
        .then((res) => {
          console.log("change client email success", res);
          tryRedirectTo(
            `${window.location.origin}/products/people/view/@self?email_change=success`
          );
        })
        .catch((e) => {
          console.log("change client email error", e);
        });
    } else {
      tryRedirectTo(defaultPage);
    }
  }

  render() {
    console.log("Change email render");
    return <Loader className="pageLoader" type="rombs" size="40px" />;
  }
}

ChangeEmail.propTypes = {
  location: PropTypes.object.isRequired,
  changeEmail: PropTypes.func.isRequired,
};
const ChangeEmailForm = (props) => (
  <PageLayout>
    <PageLayout.SectionBody>
      <ChangeEmail {...props} />
    </PageLayout.SectionBody>
  </PageLayout>
);

export default inject(({ auth }) => {
  const { logout, userStore, settingsStore, isLoaded } = auth;
  return {
    isLoaded,
    userId: userStore.user.id,
    logout,
    changeEmail: userStore.changeEmail,
    defaultPage: settingsStore.defaultPage,
  };
})(observer(withRouter(ChangeEmailForm)));
