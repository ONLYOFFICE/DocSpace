import React from "react";
import { withRouter } from "react-router";
import { Box } from "asc-web-components";
import { utils, Loaders, ErrorContainer } from "asc-web-common";

const { getObjectByLocation, showLoader, hideLoader } = utils;

class ThirdPartyResponse extends React.Component {
  constructor(props) {
    super(props);
    const { provider } = props.match.params;
    const urlParams = getObjectByLocation(window.location);

    this.code = urlParams ? urlParams.code || null : null;
    this.error = urlParams ? urlParams.error || null : null;
    this.provider = provider;
  }

  componentDidMount() {
    showLoader();

    if (this.code) {
      localStorage.setItem("code", this.code);
      hideLoader();
      window.close();
    } else {
      hideLoader();
    }
  }

  render() {
    return (
      <Box>
        {!this.error ? (
          <Loaders.Rectangle height="100vh" width="100vw" />
        ) : (
          <ErrorContainer bodyText={this.error} />
        )}
      </Box>
    );
  }
}

export default withRouter(ThirdPartyResponse);
