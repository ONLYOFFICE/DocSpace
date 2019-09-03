import React from 'react';
import { withRouter } from "react-router";

const Confirm = (props) => {
    const { match } = props;

    const matchStr = JSON.stringify(match);

    return (
        <span>{matchStr}</span>
    );
}

export default withRouter(Confirm);