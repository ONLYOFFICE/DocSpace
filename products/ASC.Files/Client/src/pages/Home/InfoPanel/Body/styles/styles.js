import styled from "styled-components";

const StyledInfoRoomBody = styled.div`
    padding: 0px 16px 16px;
    .no-item {
        text-align: center;
    }

    .no-thumbnail-img-wrapper {
        padding: 9px 0 34px;
        height: auto;
        width: 100%;
        display: flex;
        justify-content: center;
        .no-thumbnail-img {
            height: 96px;
            width: 96px;
        }
    }
`;

const StyledTitle = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    height: auto;
    padding: 24px 0;

    .icon {
        svg {
            height: 32px;
            width: 32px;
        }
    }

    .text {
        font-family: Open Sans;
        line-height: 22px;
        margin: 0 8px;
    }
`;

const StyledThumbnail = styled.div`
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    height: 200px;
    padding: 4px;
    border: solid 1px #eceef1;
    border-radius: 6px;
    margin-bottom: 24px;
    img {
        max-height: 200px;
        max-width: 100%;
        width: auto;
        height: auto;
    }
`;

const StyledSubtitle = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    margin-bottom: 24px;
`;

const StyledProperties = styled.div`
    display: flex;
    flex-direction: column;
    width: 100%;
    margin-bottom: 24px;
    gap: 8px;

    .property {
        width: 100%;
        display: grid;
        grid-template-columns: 135px 1fr;
        grid-column-gap: 24px;

        .property-title {
            font-size: 13px;
            color: #333333;
        }

        .property-content {
            display: flex;
            align-items: center;

            font-weight: 600;
            font-size: 13px;
            color: #333333;
        }
    }
`;

const StyledAccess = styled.div`
    display: flex;
    flex-wrap: wrap;
    flex-direction: row;
    gap: 8px;
    align-items: center;
    .divider {
        background: #eceef1;
        margin: 2px 4px;
        width: 1px;
        height: 28px;
    }

    .show-more-users {
        position: static;
        width: 101px;
        height: 16px;
        left: 120px;
        top: 8px;

        font-family: Open Sans;
        font-style: normal;
        font-weight: normal;
        font-size: 12px;
        line-height: 16px;
        text-align: right;

        color: #a3a9ae;

        flex: none;
        order: 3;
        flex-grow: 0;

        cursor: pointer;
        &:hover {
            text-decoration: underline;
        }
    }
`;

const StyledAccessUser = styled.div`
    width: 32px;
    height: 32px;
    border-radius: 50%;

    a {
        img {
            border-radius: 50%;
            width: 100%;
            height: 100%;
        }
    }
`;

const StyledOpenSharingPanel = styled.div`
    position: static;
    width: auto;
    height: 15px;
    left: 0px;
    top: 2px;

    font-family: Open Sans;
    font-style: normal;
    font-weight: 600;
    font-size: 13px;
    line-height: 15px;

    color: #3b72a7;

    flex: none;
    order: 0;
    flex-grow: 0;
    margin: 16px 0px;

    cursor: pointer;
    &:hover {
        text-decoration: underline;
        text-decoration-style: dashed;
    }
`;

export {
    StyledInfoRoomBody,
    StyledTitle,
    StyledThumbnail,
    StyledSubtitle,
    StyledProperties,
    StyledAccess,
    StyledAccessUser,
    StyledOpenSharingPanel,
};
