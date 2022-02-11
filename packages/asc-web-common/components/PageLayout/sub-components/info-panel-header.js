import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import { tablet } from "@appserver/components/utils/device";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import React from "react";
import styled from "styled-components";

const SubInfoPanelHeader = ({ children, toggleIsVisible }) => {
    const content = children.props.children;

    console.log(toggleIsVisible);

    const StyledInfoPanelHeader = styled.div`
        width: 100%;
        height: 52px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin: 8px -16px;

        .close-btn {
            @media ${tablet} {
                display: none;
            }
        }
    `;

    const StyledDivider = styled.div`
        margin: 0 -16px;
        width: calc(100% + 32px);
        height: 1px;
        background: #eceef1;
    `;

    return (
        <>
            <StyledInfoPanelHeader>
                <Text className="header-text" fontSize="21px" fontWeight="700">
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
            </StyledInfoPanelHeader>
            <StyledDivider />
        </>
    );
};

SubInfoPanelHeader.displayName = "SubInfoPanelHeader";

SubInfoPanelHeader.propTypes = {
    children: PropTypes.oneOfType([
        PropTypes.arrayOf(PropTypes.node),
        PropTypes.node,
        PropTypes.any,
    ]),
    toggleIsVisible: PropTypes.func,
};

export default inject(({ infoPanelStore }) => {
    let toggleIsVisible = () => {};
    if (infoPanelStore) {
        toggleIsVisible = infoPanelStore.toggleIsVisible;
    }
    return { toggleIsVisible };
})(observer(SubInfoPanelHeader));
