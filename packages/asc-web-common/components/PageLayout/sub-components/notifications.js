import React from "react";
import styled from "styled-components";

const Notifications = ({ children }) => {
    const StyledNotifications = styled.div`
        height: 100%;
        width: 400px;
        background-color: red;
    `;

    console.log("N - ", children);

    return <StyledNotifications>{children}</StyledNotifications>;
};

export default Notifications;
