import React from 'react'
import PropTypes from 'prop-types'
import styled, { css } from 'styled-components';

const Input = props => {

    return (
        <input value={props.value} />
    );
}

Input.propTypes = {
    value: PropTypes.string
}

export default Input
