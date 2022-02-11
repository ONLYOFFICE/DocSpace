import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import { tablet } from "@appserver/components/utils/device";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import React from "react";
import styled from "styled-components";

const SubRoomInfoHeader = ({ children, toggleIsVisible }) => {
    const content = children.props.children;

    console.log(toggleIsVisible);

    const StyledRoomInfoHeader = styled.div`
        width: 100%;
        height: 53px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin: 8px 0;

        .close-btn {
            @media ${tablet} {
                display: none;
            }
        }
    `;

    return (
        <StyledRoomInfoHeader>
            <Text fontSize="21px" fontWeight="700">
                {content}
            </Text>
            <IconButton
                className="close-btn"
                onClick={toggleIsVisible}
                iconName="/static/images/cross.react.svg"
                size="17"
                color="#A3A9AE"
                hoverColor="#657077"
                isFill={true}
            />
        </StyledRoomInfoHeader>
    );
};

SubRoomInfoHeader.displayName = "SubRoomInfoHeader";

SubRoomInfoHeader.propTypes = {
    children: PropTypes.oneOfType([
        PropTypes.arrayOf(PropTypes.node),
        PropTypes.node,
        PropTypes.any,
    ]),
    toggleIsVisible: PropTypes.func,
};

export default inject(({ roomInfoStore }) => {
    let toggleIsVisible = () => {};
    if (roomInfoStore) {
        toggleIsVisible = roomInfoStore.toggleIsVisible;
    }
    return { toggleIsVisible };
})(observer(SubRoomInfoHeader));
