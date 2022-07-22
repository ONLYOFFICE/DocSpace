import React, { Component } from "react";
import PropTypes from "prop-types";
import Loader from "@docspace/components/loader";
import Section from "../Section";

export class ExternalRedirect extends Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    const { to } = this.props;
    to && window.location.replace(to);
  }

  render() {
    return (
      <Section>
        <Section.SectionBody>
          <Loader className="pageLoader" type="rombs" size="40px" />
        </Section.SectionBody>
      </Section>
    );
  }
}

ExternalRedirect.propTypes = {
  to: PropTypes.string,
};

export default ExternalRedirect;
