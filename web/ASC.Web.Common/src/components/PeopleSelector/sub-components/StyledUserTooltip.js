import styled from "styled-components";

const StyledUserTooltip = styled.div`
    width: 253px;
    min-height: 63px;
    display: "grid";
    grid-template-columns: "30px 1fr";
    grid-template-rows: "1fr";
    grid-column-gap: 8px;

    .email-text {
        padding-bottom: 8px;
    }

    .block-avatar-name {
        display: flex;
        align-items: center;
        padding-bottom: 8px;
    }

    .user-avatar {
        margin-right: 10px;
        min-width: 32px;
    }

`;

export default StyledUserTooltip