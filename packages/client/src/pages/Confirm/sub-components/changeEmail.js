import React from "react";
import PropTypes from "prop-types";
import { inject, observer } from "mobx-react";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { combineUrl } from "@docspace/common/utils";
import tryRedirectTo from "@docspace/common/utils/tryRedirectTo";

class ChangeEmail extends React.PureComponent {
  componentDidMount() {
    const { changeEmail, userId, isLoaded, linkData } = this.props;
    if (isLoaded) {
      const [email, key] = [linkData.email, linkData.confirmHeader];
      changeEmail(userId, email, key)
        .then((res) => {
          console.log("change client email success", res);
          tryRedirectTo(
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              `/profile?email_change=success`
            )
          );
        })
        .catch((error) => {
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

          console.log("change client email error", e);
          tryRedirectTo(
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              `/error=${errorMessage}`
            )
          );
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
            combineUrl(
              window.DocSpaceConfig?.proxy?.url,
              `/profile?email_change=success`
            )
          );
        })
        .catch((e) => console.log("change client email error", e));
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
  changeEmail: PropTypes.func.isRequired,
};
const ChangeEmailForm = (props) => (
  <Section>
    <Section.SectionBody>
      <ChangeEmail {...props} />
    </Section.SectionBody>
  </Section>
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
})(observer(ChangeEmailForm));
