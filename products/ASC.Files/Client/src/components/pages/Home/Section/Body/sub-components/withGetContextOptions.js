import React from 'react';

export default withGetContextOptions(WrappedComponent) => {
    return class extends React.Component {

        render() {
            return <WrappedComponent {...this.props}/>
        }
    }
}