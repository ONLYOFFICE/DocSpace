import React from "react";
import { withRouter } from "react-router";
import { Box } from "asc-web-components";
import { utils } from "asc-web-common";

const { getObjectByLocation } = utils;

class ThirdPartyResponse extends React.Component {
  constructor(props) {
    super(props);
    const { provider } = props.match.params;
    const urlParams = getObjectByLocation(window.location);

    this.code = urlParams ? urlParams.code || null : null;
    this.provider = provider;
  }

  async componentDidMount() {
    localStorage.setItem("provider", this.provider);
    localStorage.setItem("code", this.code);

    setTimeout(window.close(), 1000);
  }

  render() {
    return <Box>OK</Box>;
  }
}

export default withRouter(ThirdPartyResponse);
