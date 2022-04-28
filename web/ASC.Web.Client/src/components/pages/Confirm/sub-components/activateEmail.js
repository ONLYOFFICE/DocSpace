import React from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import Loader from "@appserver/components/loader";
import Section from "@appserver/common/components/Section";
import { combineUrl } from "@appserver/common/utils";
import tryRedirectTo from "@appserver/common/utils/tryRedirectTo";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@appserver/common/constants";

class ActivateEmail extends React.PureComponent {
  componentDidMount() {
    const { logout, changeEmail, linkData } = this.props;
    const [email, uid, key] = [
      linkData.email,
      linkData.uid,
      linkData.confirmHeader,
    ];
    logout();
    changeEmail(uid, email, key)
      .then((res) => {
        tryRedirectTo(
          combineUrl(
            AppServerConfig.proxyURL,
            `/login/confirmed-email=${email}`
          )
        );
      })
      .catch((e) => {
        // console.log('activate email error', e);
        tryRedirectTo(
          combineUrl(AppServerConfig.proxyURL, `/login/error=${e}`)
        );
      });
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
