import React, { useContext } from 'react';
import { Container, Col, Row } from 'reactstrap';
// import { useTranslation, withTranslation, Trans } from 'react-i18next';
import ModuleTile from '../../ui/ModuleTile/ModuleTile';
import ModuleContext from '../../context/ModuleContext';


const PrimaryTile = ({ modules }) => (
    <Row>
        {
            modules.filter(m => m.isPrimary).map(module => (
                <Col key={0}>
                    <ModuleTile {...module} />
                </Col>
            ))
        }
    </Row>
);

const NotPrimaryTiles = ({ modules }) => {
    let index = 0;
    return (
    <Row>
        {modules.filter(m => !m.isPrimary).map(module => (
            <Col key={++index}>
                <ModuleTile {...module} />
            </Col>
        ))
        }
    </Row>
);
};

const Home = props => {
    const context = useContext(ModuleContext);
    const { modules, isFetching } = context;

    return (
        <Container>
            {isFetching ? (
                <Row>
                    Loading...
                    </Row>
            ) : (
                    <>
                        <PrimaryTile modules={modules} />
                        <NotPrimaryTiles modules={modules} />
                    </>
                )
            }
        </Container>
    );
}

export default Home;