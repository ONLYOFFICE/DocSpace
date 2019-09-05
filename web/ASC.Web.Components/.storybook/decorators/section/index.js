import React from 'react';
import PropTypes from 'prop-types';

const sectionStyles = {
  padding: 16,
};

const Section = props => <div style={sectionStyles}>{props.children}</div>;

Section.propTypes = { children: PropTypes.node.isRequired };

export default Section;