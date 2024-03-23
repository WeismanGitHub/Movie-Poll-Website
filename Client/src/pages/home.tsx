import Navbar from '../navbar';

export default function Home() {
    return (
        <>
            <Navbar />
            <div
                className="container d-flex justify-content-center align-items-center text-center fs-4 p-2 flex-column"
                style={{ height: '75vh' }}
            >
                <h1>
                    Set up movie polls where you can limit voting to members of a particular Discord server. 
                </h1>
            </div>
        </>
    );
}
