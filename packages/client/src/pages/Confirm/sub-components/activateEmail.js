import React from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { combineUrl } from "@docspace/common/utils";
import tryRedirectTo from "@docspace/common/utils/tryRedirectTo";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@docspace/common/constants";

class ActivateEmail extends React.PureComponent {
  componentDidMount() {
    const { logout, changeEmail, linkData } = this.props;
    const [email, uid, key] = [
      linkData.email,
      linkData.uid,
      linkData.confirmHeader,
    ];
    logout().then(() =>
      changeEmail(uid, email, key)
        .then((res) => {
          tryRedirectTo(
            combineUrl(
              AppServerConfig.proxyURL,
              `/login?confirmedEmail=${email}`
            )
          );
        })
        .catch((error) => {
          // console.log('activate email error', e);
          let errorMessage = "";
          if (typeof error === "object") {
            errorMessage =
              error?.response?.data?.error?.message ||
              error?.statusText ||
              error?.message ||
              "";
          } else {
            errorMessage = error;
          }

          tryRedirectTo(
            combineUrl(
              AppServerConfig.proxyURL,
              `/login/error?message=${errorMessage}`
            )
          );
        })
    );
  }

  render() {
    // console.log('Activate email render');
    return <Loader className="pageLoader" type="rombs" size="40px" />;
  }
}

ActivateEmail.propTypes = {
  location: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
};
const ActivateEmailForm = (props) => (
  <Section>
    <Section.SectionBody>
      <ActivateEmail {...props} />
    </Section.SectionBody>
  </Section>
);

export default inject(({ auth }) => {
  const { logout, userStore } = auth;
  return {
    logout,
    changeEmail: userStore.changeEmail,
  };
})(withRouter(observer(ActivateEmailForm)));
