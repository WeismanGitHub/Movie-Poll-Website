import Navbar from '../navbar';

export default function Home() {
    return (
        <>
            <Navbar />
            <div className="container justify-content-center align-items-center vh-100 vw-100 text-center fs-4 p-2 d-flex">
                <div>
                    <h1>Easily create polls and engage in a collective movie-selection process.</h1>
                    Integrate seamlessly with Discord to restrict polls to server members.
                    <br />
                    <div>
                        Built with <strong>C#</strong>, <strong>Typescript</strong>, <strong>ASP.NET</strong>,{' '}
                        <strong>SQL</strong>, and <strong>React</strong>.
                    </div>
                    <br />
                </div>
            </div>
        </>
    );
}
