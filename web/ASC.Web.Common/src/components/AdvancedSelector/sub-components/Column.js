import React from "react";
import PropTypes from "prop-types";
import StyledColumn from "./StyledColumn";

class Column extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        const { children, displayType, className, style, size } = this.props;
        return (
            <StyledColumn displayType={displayType} className={className} style={style} size={size}>
                {children}
            </StyledColumn>
        );
    }
}

Column.propTypes = {
    children: PropTypes.any,
    className: PropTypes.string,
    style: PropTypes.object,
    displayType: PropTypes.oneOf(["dropdown", "aside"]),
    size: PropTypes.oneOf(["compact", "full"])
};

export default Column;