import React from "react";
import PropTypes from 'prop-types';
import StyledHeading from "./StyledHeadline";

const Headline = (props) => {
   //console.log("Headline render");
   return (
      <StyledHeading
         {...props} />
   );
};

Headline.propTypes = {
   level: PropTypes.oneOf([1, 2, 3, 4, 5, 6]),
   children: PropTypes.any,
   color: PropTypes.string,
   title: PropTypes.string,
   truncate: PropTypes.bool,
   isInline: PropTypes.bool,
   type: PropTypes.oneOf(['content', 'header', 'menu']),
};

Headline.defaultProps = {
   color: '#333333',
   title: '',
   truncate: false,
   isInline: false,
   level: 1
};

export default Headline;