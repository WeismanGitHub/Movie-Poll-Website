import Navbar from '../navbar';

export default function Home() {
    return (
        <>
            <Navbar />
            <div
                className="container d-flex justify-content-center align-items-center text-center fs-4 p-2 flex-column"
                style={{ height: '75vh' }}
            >
                <h4>
                    Create a poll where users vote for a movie from a pre-defined list. Additionally, set an
                    expiration date and restrict it to members of a specific Discord server.
                </h4>
                <img src=""></img>
            </div>
        </>
    );
}
