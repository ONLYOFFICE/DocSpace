import React, { Component } from "react";
import PropTypes from "prop-types";
import { PageLayout, Loader } from "asc-web-components";

export class ExternalRedirect extends Component {
  constructor(props) {
    super();
    this.state = {
      location: props.to
    };
  }

  componentDidMount() {
    window.location.replace(this.state.location);
  }

  render() {
    return <PageLayout
                sectionBodyContent={
                    <Loader className="pageLoader" type="rombs" size={40} />
                }
            />
  }
}

ExternalRedirect.propTypes = {
  to: PropTypes.string
}

export default ExternalRedirect;
