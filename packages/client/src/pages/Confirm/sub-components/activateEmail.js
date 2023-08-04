import React from "react";
import PropTypes from "prop-types";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { combineUrl } from "@docspace/common/utils";
import tryRedirectTo from "@docspace/common/utils/tryRedirectTo";
import { inject, observer } from "mobx-react";
import { EmployeeActivationStatus } from "@docspace/common/constants";

class ActivateEmail extends React.PureComponent {
  componentDidMount() {
    const { logout, updateEmailActivationStatus, linkData } = this.props;
    const [email, uid, key] = [
      linkData.email,
      linkData.uid,
      linkData.confirmHeader,
    ];
    logout().then(() =>
      updateEmailActivationStatus(EmployeeActivationStatus.Activated, uid, key)
        .then((res) => {
          tryRedirectTo(
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
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
              window.DocSpaceConfig?.proxy?.url,
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
    updateEmailActivationStatus: userStore.updateEmailActivationStatus,
  };
})(observer(ActivateEmailForm));
