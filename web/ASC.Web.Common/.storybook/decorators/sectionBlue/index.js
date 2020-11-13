import React from "react";
import PropTypes from "prop-types";

const sectionStyles = {
  padding: 16,
  background: "#0F4071",
  color: "#fff",
};

const Section = (props) => <div style={sectionStyles}>{props.children}</div>;

Section.propTypes = { children: PropTypes.node.isRequired };

export default Section;
