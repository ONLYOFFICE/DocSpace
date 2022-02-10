import IconButton from "@appserver/components/icon-button";
import { mobile, tablet } from "@appserver/components/utils/device";
import { inject } from "mobx-react";
import React from "react";
import styled from "styled-components";

const RoomInfo = ({ children, isVisible, toggleIsVisible }) => {
    if (!isVisible) return null;

    const StyledRoomInfoWrapper = styled.div`
        height: auto;
        width: auto;
        background: rgba(6, 22, 38, 0.2);
        backdrop-filter: blur(18px);

        @media ${tablet} {
            z-index: 191;
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
        }
    `;

    const StyledRoomInfo = styled.div`
        height: 100%;
        width: 368px;
        background-color: #ffffff;
        border-left: 1px solid #eceef1;
        display: flex;
        flex-direction: column;
        align-items: center;

        @media ${tablet} {
            position: absolute;
            border: none;
            right: 0;
            width: 480px;
            max-width: calc(100vw - 37px);
        }

        @media ${mobile} {
            bottom: 0;
            height: 80%;
            max-width: none;
            width: 100%;
        }
    `;

    const StyledCloseButtonWrapper = styled.div`
        position: absolute;
        display: none;
        @media ${tablet} {
            display: block;
            top: 0;
            left: 0;
            margin-top: 18px;
            margin-left: -27px;
        }
        @media ${mobile} {
            right: 0;
            left: auto;
            margin-top: -27px;
            margin-right: 10px;
        }
    `;

    return (
        <StyledRoomInfoWrapper>
            <StyledRoomInfo>
                <StyledCloseButtonWrapper>
                    <IconButton
                        onClick={toggleIsVisible}
                        iconName="/static/images/arrow.path.react.svg"
                        size="17"
                        color="#ffffff"
                        hoverColor="#657077"
                        isFill={true}
                        className="room-info-button"
                    />
                </StyledCloseButtonWrapper>
                {children}
            </StyledRoomInfo>
        </StyledRoomInfoWrapper>
    );
};

export default inject(({ roomInfoStore }) => {
    let isVisible = false;
    let toggleIsVisible = () => {};
    if (roomInfoStore) {
        isVisible = roomInfoStore.isVisible;
        toggleIsVisible = roomInfoStore.toggleIsVisible;
    }
    return {
        isVisible,
        toggleIsVisible,
    };
})(RoomInfo);
