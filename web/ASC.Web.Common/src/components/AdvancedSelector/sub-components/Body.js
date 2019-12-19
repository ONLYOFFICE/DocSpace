import React from "react";
import PropTypes from "prop-types";
import StyledBody from "./StyledBody";

class Body extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        const { children, displayType, className, style } = this.props;
        return (
            <StyledBody displayType={displayType} className={className} style={style}>
                {children}
            </StyledBody>
        );
    }
}

Body.propTypes = {
    children: PropTypes.any,
    className: PropTypes.string,
    style: PropTypes.object,
    displayType: PropTypes.oneOf(["dropdown", "aside"]),
};

export default Body;