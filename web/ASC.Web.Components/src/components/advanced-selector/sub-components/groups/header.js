import React from "react";
import PropTypes from "prop-types";
import Text from "../../../text";

const ADSelectorGroupsHeader = (props) => {
    const {headerLabel} = props;
    return (
        <Text as="p" className="group_header" fontSize={15} isBold={true}>
          {headerLabel}
        </Text>
    );
}

ADSelectorGroupsHeader.propTypes = {
    headerLabel: PropTypes.string
}

export default ADSelectorGroupsHeader;